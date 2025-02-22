//
//  StatsView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI

struct StatsView: View {
    @State private var totalWords = 0
    @State private var averageConfidence = 0.0
    @State private var wordsLearned = 0
    
    var body: some View {
        NavigationView {
            List {
                StatRow(title: "Total Words", value: "\(totalWords)")
                StatRow(title: "Words Mastered", value: "\(wordsLearned)")
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
