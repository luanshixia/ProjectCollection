//
//  StatsView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI
import SwiftData

struct StatsView: View {
    @Query private var words: [Word]
    
    var totalWords: Int {
        words.count
    }
    
    var averageConfidence: Double {
        guard totalWords > 0 else { return 0 }
        let sum = words.reduce(0) { $0 + Double($1.confidence) }
        return sum / Double(totalWords)
    }
    
    var masteredWords: Int {
        words.filter { $0.confidence >= 4 }.count
    }
    
    var body: some View {
        NavigationView {
            List {
                StatRow(title: "Total Words", value: "\(totalWords)")
                StatRow(title: "Words Mastered", value: "\(masteredWords)")
                StatRow(title: "Average Confidence", value: String(format: "%.1f", averageConfidence))
            }
            .navigationTitle("Statistics")
        }
    }
}

struct StatRow: View {
    let title: String
    let value: String
    
    var body: some View {
        HStack {
            Text(title)
            Spacer()
            Text(value)
                .foregroundColor(.secondary)
        }
    }
}
