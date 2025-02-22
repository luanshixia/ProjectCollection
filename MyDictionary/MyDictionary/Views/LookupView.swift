//
//  LookupView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI

struct LookupView: View {
    @State private var searchText = ""
    @State private var showDictionary = false
    @State private var savedWords: [WordEntry] = []
    @Environment(\.managedObjectContext) private var viewContext
    
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
                
                // Recently looked up words
                List {
                    Section(header: Text("Recent Lookups")) {
                        ForEach(savedWords) { word in
                            WordRow(word: word)
                        }
                    }
                }
            }
            .navigationTitle("Dictionary")
        }
    }
    
    func lookupWord() {
        guard !searchText.isEmpty else { return }
        
        if UIReferenceLibraryViewController.dictionaryHasDefinition(forTerm: searchText) {
            // Save word to history
            let newWord = WordEntry(
                word: searchText,
                dateAdded: Date(),
                lastReviewed: nil,
                reviewCount: 0,
                confidence: 1
            )
            savedWords.insert(newWord, at: 0)
            
            // Show system dictionary
            let dictionary = UIReferenceLibraryViewController(term: searchText)
            if let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
               let viewController = windowScene.windows.first?.rootViewController {
                viewController.present(dictionary, animated: true)
            }
        }
    }
}

struct WordRow: View {
    let word: WordEntry
    
    var body: some View {
        HStack {
            VStack(alignment: .leading) {
                Text(word.word)
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
