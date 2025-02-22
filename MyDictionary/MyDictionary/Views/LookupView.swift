//
//  LookupView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI
import SwiftData

struct LookupView: View {
    @Environment(\.modelContext) private var modelContext
    @Query(sort: \Word.dateAdded, order: .reverse) private var words: [Word]
    @State private var searchText = ""
    @State private var suggestions: [String] = []
    
    var groupedWords: [(String, [Word])] {
        let calendar = Calendar.current
        let now = Date()
        
        return Dictionary(grouping: words) { word in
            let components = calendar.dateComponents([.day], from: word.dateAdded, to: now)
            if let days = components.day {
                switch days {
                case 0:
                    return "Today"
                case 1:
                    return "Yesterday"
                case 2...7:
                    return "This Week"
                case 8...14:
                    return "Last Week"
                case 15...30:
                    return "This Month"
                default:
                    let dateFormatter = DateFormatter()
                    dateFormatter.dateFormat = "MMMM yyyy"
                    return dateFormatter.string(from: word.dateAdded)
                }
            }
            return "Unknown"
        }
        .sorted { group1, group2 in
            let order = ["Today", "Yesterday", "This Week", "Last Week", "This Month"]
            let index1 = order.firstIndex(of: group1.0) ?? Int.max
            let index2 = order.firstIndex(of: group2.0) ?? Int.max
            if index1 == index2 {
                return group1.0 < group2.0
            }
            return index1 < index2
        }
    }
    
    var body: some View {
        NavigationView {
            VStack {
                // Search bar with suggestions
                VStack {
                    HStack {
                        TextField("Enter a word", text: $searchText)
                            .textFieldStyle(RoundedBorderTextFieldStyle())
                            .textInputAutocapitalization(.never)
                            .autocorrectionDisabled(true)
                            .onChange(of: searchText) { oldValue, newValue in
                                updateSuggestions(for: newValue)
                            }
                        
                        Button(action: {
                            if !searchText.isEmpty {
                                lookupWord(searchText)
                            }
                        }) {
                            Image(systemName: "magnifyingglass")
                                .foregroundColor(.blue)
                        }
                    }
                    .padding()
                    
                    // Suggestions dropdown
                    if !suggestions.isEmpty && !searchText.isEmpty {
                        List(suggestions, id: \.self) { suggestion in
                            Text(suggestion)
                                .onTapGesture {
                                    searchText = suggestion
                                    lookupWord(suggestion)
                                    suggestions = []
                                }
                        }
                        .frame(maxHeight: 200)
                        .background(Color(.systemBackground))
                    }
                }
                
                // Grouped word list
                List {
                    ForEach(groupedWords, id: \.0) { group in
                        Section(header: Text(group.0)) {
                            ForEach(group.1) { word in
                                WordRow(word: word)
                                    .contentShape(Rectangle())
                                    .onTapGesture {
                                        showDictionary(for: word.text)
                                    }
                            }
                            .onDelete { indexSet in
                                delete(words: group.1, at: indexSet)
                            }
                        }
                    }
                }
            }
            .navigationTitle("Dictionary")
        }
    }
    
    private func updateSuggestions(for text: String) {
        guard !text.isEmpty else {
            suggestions = []
            return
        }
        
        // Use UITextChecker for word suggestions
        let checker = UITextChecker()
        let range = NSRange(location: 0, length: text.utf16.count)
        let completions = checker.completions(forPartialWordRange: range,
                                              in: text,
                                              language: "en") ?? []
        suggestions = completions.prefix(5).map { text + $0 }
        
        // Add dictionary validations
        suggestions = suggestions.filter { suggestion in
            UIReferenceLibraryViewController.dictionaryHasDefinition(forTerm: suggestion)
        }
    }
    
    private func showDictionary(for word: String) {
        let dictionary = UIReferenceLibraryViewController(term: word)
        if let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
           let viewController = windowScene.windows.first?.rootViewController {
            viewController.present(dictionary, animated: true)
        }
    }
    
    private func lookupWord(_ word: String) {
        if UIReferenceLibraryViewController.dictionaryHasDefinition(forTerm: word) {
            let newWord = Word(text: word)
            modelContext.insert(newWord)
            
            showDictionary(for: word)
            searchText = ""
            suggestions = []
        }
    }
    
    private func delete(words: [Word], at indexes: IndexSet) {
        for index in indexes {
            modelContext.delete(words[index])
        }
    }
}

struct WordRow: View {
    let word: Word
    
    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text(word.text)
                .font(.headline)
            
            Text("Added: \(word.formattedDateAdded)")
                .font(.caption)
                .foregroundColor(.secondary)
            
            ConfidenceBadge(level: word.confidence)
            
            Text("Next review: \(word.formattedNextReview)")
                .font(.caption)
                .foregroundColor(.secondary)
        }
        .padding(.vertical, 4)
    }
}

struct ConfidenceBadge: View {
    let level: Int
    
    var body: some View {
        Text("\(level)")
            .font(.caption)
            .padding(8)
            .background(confidenceColor)
            .foregroundColor(.white)
            .clipShape(Circle())
    }
    
    var confidenceColor: Color {
        switch level {
        case 1: return .red
        case 2: return .orange
        case 3: return .yellow
        case 4: return .green
        case 5: return .blue
        default: return .gray
        }
    }
}

struct ConfidenceButton: View {
    let level: Int
    let action: () -> Void
    
    var body: some View {
        Button(action: action) {
            Text("\(level)")
                .font(.headline)
                .frame(width: 44, height: 44)
                .background(ConfidenceBadge(level: level).confidenceColor)
                .foregroundColor(.white)
                .clipShape(Circle())
        }
    }
}
