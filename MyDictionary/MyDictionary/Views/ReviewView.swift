//
//  ReviewView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI
import SwiftData

struct ReviewView: View {
    @Environment(\.modelContext) private var modelContext
    @Query private var words: [Word]
    @State private var currentWord: Word?
    
    var body: some View {
        NavigationView {
            VStack {
                if let word = currentWord {
                    ReviewCard(
                        word: word,
                        onConfidence: recordConfidence)
                } else {
                    Text("No words to review!")
                        .font(.title)
                }
            }
            .navigationTitle("Review")
            .onAppear {
                fetchNextWordToReview()
            }
        }
    }
    
    private func fetchNextWordToReview() {
        let now = Date()
        currentWord = words.first { word in
            guard let nextReview = word.nextReviewDate else { return true }
            return nextReview <= now
        }
    }
    
    private func recordConfidence(_ level: Int) {
        guard let word = currentWord else { return }
        
        // Calculate next review interval using SM-2
        let (interval, _) = calculateNextReview(
            quality: level,
            reviewCount: word.reviewCount,
            previousInterval: daysSinceLastReview(word)
        )
        
        // Update word
        word.confidence = level
        word.reviewCount += 1
        word.lastReviewed = Date()
        word.nextReviewDate = Calendar.current.date(
            byAdding: .day,
            value: Int(interval),
            to: Date()
        )
        
        // Reset for next word
        fetchNextWordToReview()
    }
    
    private func calculateNextReview(
        quality: Int,
        reviewCount: Int,
        previousInterval: Int
    ) -> (interval: Double, easeFactor: Double) {
        let baseEaseFactor: Double = 2.5
        let minEaseFactor: Double = 1.3
        var interval: Double
        var easeFactor = baseEaseFactor
        
        switch reviewCount {
        case 0:
            interval = 1.0
        case 1:
            interval = 6.0
        default:
            let qualityFactor = (0.1 - (5 - Double(quality)) * (0.08 + (5 - Double(quality)) * 0.02))
            easeFactor = baseEaseFactor + qualityFactor
            easeFactor = max(minEaseFactor, easeFactor)
            interval = Double(previousInterval) * easeFactor
        }
        
        if quality < 3 {
            interval = 1.0
        }
        
        return (interval, easeFactor)
    }
    
    private func daysSinceLastReview(_ word: Word) -> Int {
        guard let lastReviewed = word.lastReviewed else { return 0 }
        return max(1, Calendar.current.dateComponents([.day], from: lastReviewed, to: Date()).day ?? 1)
    }
}

struct ReviewCard: View {
    let word: Word
    let onConfidence: (Int) -> Void
    
    var body: some View {
        VStack(spacing: 24) {
            Text(word.text)
                .font(.largeTitle)
            
            Button(action: {
                let dictionary = UIReferenceLibraryViewController(term: word.text)
                if let windowScene = UIApplication.shared.connectedScenes.first as? UIWindowScene,
                   let viewController = windowScene.windows.first?.rootViewController {
                    viewController.present(dictionary, animated: true)
                }
            }) {
                Text("Show Definition")
                    .foregroundColor(.blue)
            }
            
            HStack(spacing: 12) {
                ForEach(1...5, id: \.self) { level in
                    ConfidenceButton(level: level) {
                        onConfidence(level)
                    }
                }
            }
        }
        .frame(maxWidth: .infinity)
        .padding(.vertical, 32)
        .background(Color(.systemBackground))
        .cornerRadius(12)
        .shadow(radius: 5)
        .padding()
    }
}
