//
//  ReviewView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI

struct ReviewView: View {
    @State private var wordsToReview: [WordEntry] = []
    @State private var currentIndex = 0
    
    var body: some View {
        NavigationView {
            if wordsToReview.isEmpty {
                VStack {
                    Text("No words to review!")
                        .font(.title)
                    Text("Look up some words first")
                        .foregroundColor(.secondary)
                }
            } else {
                VStack {
                    // Review card
                    ReviewCard(word: wordsToReview[currentIndex])
                    
                    // Confidence buttons
                    HStack {
                        ForEach(1...5, id: \.self) { level in
                            ConfidenceButton(level: level) {
                                recordConfidence(level)
                            }
                        }
                    }
                    .padding()
                }
                .navigationTitle("Review")
            }
        }
    }
    
    func recordConfidence(_ level: Int) {
        // Update word confidence and schedule next review
        // Implementation would use spaced repetition algorithm
    }
}

struct ReviewCard: View {
    let word: WordEntry
    @State private var showDefinition = false
    
    var body: some View {
        VStack {
            Text(word.word)
                .font(.largeTitle)
                .padding()
            
            Button(action: { showDefinition.toggle() }) {
                Text(showDefinition ? "Hide Definition" : "Show Definition")
            }
            .padding()
            
            if showDefinition {
                // This would show a cached definition or trigger system dictionary
                Text("Tap to view in dictionary")
                    .foregroundColor(.blue)
            }
        }
        .frame(maxWidth: .infinity, maxHeight: 300)
        .background(Color(.systemBackground))
        .cornerRadius(12)
        .shadow(radius: 5)
        .padding()
    }
}
