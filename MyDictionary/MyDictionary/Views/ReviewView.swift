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
    @Query private var allWords: [Word]
    @State private var wordsToReview: [Word] = []
    @State private var currentIndex = 0
    @State private var offset = CGSize.zero
    @State private var isRemovingCard = false
    
    var body: some View {
        NavigationStack {
            ZStack {
                if wordsToReview.isEmpty {
                    ContentUnavailableView("No Words to Review",
                                           systemImage: "book.closed",
                                           description: Text("Look up some words first"))
                } else {
                    // Stack of review cards
                    ForEach(Array(wordsToReview.enumerated().prefix(3)), id: \.element.id) { index, word in
                        let isTopCard = index == currentIndex
                        let offset = isTopCard ? self.offset : CGSize.zero
                        
                        cardView(for: word, at: index)
                            .zIndex(Double(wordsToReview.count - index))
                            .offset(
                                x: isTopCard ? offset.width : 0,
                                y: isTopCard ? offset.height : 0
                            )
                            .scaleEffect(calculateScale(for: index))
                            .offset(y: calculateYOffset(for: index))
                            .opacity(isRemovingCard && isTopCard ? 0 : 1)
                    }
                }
            }
            .navigationTitle("Review")
            .onAppear {
                fetchWordsToReview()
            }
        }
    }
    
    private func cardView(for word: Word, at index: Int) -> some View {
        ReviewCard(word: word, onConfidence: { confidence in
            withAnimation(.spring()) {
                isRemovingCard = true
            }
            
            // Short delay to allow animation to complete
            DispatchQueue.main.asyncAfter(deadline: .now() + 0.3) {
                recordConfidence(confidence, for: word)
                moveToNextCard()
            }
        })
        .shadow(color: Color.primary.opacity(0.2), radius: calculateShadow(for: index), x: 0, y: 2)
    }
    
    private func calculateScale(for index: Int) -> CGFloat {
        let diff = CGFloat(index - currentIndex)
        if diff <= 0 {
            return 1.0 // Current card is full size
        } else {
            return 1.0 - diff * 0.05 // Each card behind is 5% smaller
        }
    }
    
    private func calculateYOffset(for index: Int) -> CGFloat {
        let diff = CGFloat(index - currentIndex)
        if diff <= 0 {
            return 0 // Current card is at default position
        } else {
            return diff * 10 // Stack cards with increasing offset
        }
    }
    
    private func calculateShadow(for index: Int) -> CGFloat {
        let diff = CGFloat(index - currentIndex)
        if diff <= 0 {
            return 5 // Current card has normal shadow
        } else {
            return max(1, 5 - diff * 2) // Decreasing shadow for cards behind
        }
    }
    
    private func moveToNextCard() {
        // Remove the current card
        if !wordsToReview.isEmpty {
            wordsToReview.remove(at: currentIndex)
        }
        
        // Reset animation state
        offset = .zero
        isRemovingCard = false
        
        // If we're out of cards, fetch more
        if wordsToReview.isEmpty {
            fetchWordsToReview()
        }
    }
    
    private func fetchWordsToReview() {
        let now = Date()
        // Get all words that are due for review
        wordsToReview = allWords.filter { word in
            guard let nextReview = word.nextReviewDate else { return true }
            return nextReview <= now
        }
        
        // Limit to 10 words at a time to keep it manageable
        if wordsToReview.count > 10 {
            wordsToReview = Array(wordsToReview.prefix(10))
        }
        
        // Sort by review date (oldest first)
        wordsToReview.sort { (word1, word2) -> Bool in
            let date1 = word1.nextReviewDate ?? .distantPast
            let date2 = word2.nextReviewDate ?? .distantPast
            return date1 < date2
        }
    }
    
    private func recordConfidence(_ level: Int, for word: Word) {
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
    
    @State private var dragOffset = CGSize.zero
    @State private var draggedDirection: DragDirection = .none
    
    private enum DragDirection {
        case none, up, down
    }
    
    var body: some View {
        VStack(spacing: 24) {
            Text(word.text)
                .font(.largeTitle)
                .padding(.top, 8)
            // Apply a slight rotation based on drag offset for a more dynamic feel
                .rotationEffect(Angle(degrees: -dragOffset.height * 0.05))
            
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
            .padding(.bottom, 8)
        }
        .frame(maxWidth: .infinity)
        .padding(.vertical, 16)
        .background(
            RoundedRectangle(cornerRadius: 12)
                .fill(Color(.systemBackground))
                .shadow(color: Color.primary.opacity(0.3), radius: 8, x: 0, y: 2)
        )
        .offset(y: dragOffset.height)
        .rotation3DEffect(
            Angle(degrees: Double(dragOffset.height) * 0.1),
            axis: (x: 1.0, y: 0.0, z: 0.0)
        )
        .gesture(
            DragGesture()
                .onChanged { gesture in
                    // Limit drag to vertical axis and restrict the range
                    let newHeight = gesture.translation.height
                    let clampedHeight = min(max(newHeight, -60), 60)
                    dragOffset = CGSize(width: 0, height: clampedHeight)
                    
                    // Update drag direction for visual feedback
                    draggedDirection = dragOffset.height < 0 ? .up : .down
                }
                .onEnded { _ in
                    // Reset with spring animation for a bounce effect
                    withAnimation(.spring(response: 0.4, dampingFraction: 0.6)) {
                        dragOffset = .zero
                        draggedDirection = .none
                    }
                }
        )
        .overlay(
            // Visual indicator based on drag direction
            Group {
                if draggedDirection == .up {
                    Image(systemName: "arrow.up.circle")
                        .font(.system(size: 28))
                        .foregroundColor(.blue)
                        .opacity(min(abs(dragOffset.height) / CGFloat(60), 1.0))
                        .offset(y: -80)
                } else if draggedDirection == .down {
                    Image(systemName: "arrow.down.circle")
                        .font(.system(size: 28))
                        .foregroundColor(.orange)
                        .opacity(min(dragOffset.height / CGFloat(60), 1.0))
                        .offset(y: 80)
                }
            }
        )
    }
}
