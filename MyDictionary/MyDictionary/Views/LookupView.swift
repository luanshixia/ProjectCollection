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
    
    var body: some View {
        NavigationView {
            VStack {
                // Search bar
                HStack {
                    TextField("Enter a word", text: $searchText)
                        .textFieldStyle(RoundedBorderTextFieldStyle())
                        .textInputAutocapitalization(.never)
                        .autocorrectionDisabled(true)
                    
                    Button(action: lookupWord) {
                        Image(systemName: "magnifyingglass")
                            .foregroundColor(.blue)
                    }
                }
                .padding()
                
                // Word list
                List {
                    ForEach(words) { word in
                        WordRow(word: word)
                    }
                    .onDelete(perform: deleteWords)
                }
            }
            .navigationTitle("Dictionary")
        }
    }
    
    func lookupWord() {
        guard !searchText.isEmpty else { return }
        
        if UIReferenceLibraryViewController.dictionaryHasDefinition(forTerm: searchText) {
            let newWord = Word(text: searchText)
            modelContext.insert(newWord)
            searchText = ""
            
            // Show system dictionary
            let dictionary = UIReferenceLibraryViewController(term: searchText)
            if let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
               let viewController = windowScene.windows.first?.rootViewController {
                viewController.present(dictionary, animated: true)
            }
        }
    }
    
    func deleteWords(_ indexSet: IndexSet) {
        for index in indexSet {
            modelContext.delete(words[index])
        }
    }
}

struct WordRow: View {
    let word: Word
    
    var body: some View {
        HStack {
            VStack(alignment: .leading) {
                Text(word.text)
                    .font(.headline)
                Text("Added \(word.dateAdded, style: .date)")
                    .font(.caption)
                    .foregroundColor(.secondary)
            }
            
            Spacer()
            
            ConfidenceBadge(level: word.confidence)
        }
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
