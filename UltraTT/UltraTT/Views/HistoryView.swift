//
//  HistoryView.swift
//  UltraTT
//
//  Created by Yang Wang on 2/2/25.
//

import SwiftUI

struct HistoryView: View {
    @AppStorage("quizRecords") private var storedRecords = Data()
    
    var body: some View {
        VStack {
            // Optional: Add a button to view records
            Button("View Records") {
                if let records = try? JSONDecoder().decode([QuizRecord].self, from: storedRecords) {
                    print("Total records:", records.count)
                    records.forEach { record in
                        print("""
                            Question: \(record.question)
                            Answer: \(record.userAnswer)
                            Attempts: \(record.numAttempts)
                            Correct: \(record.isCorrect)
                            Time: \(record.timeSpent)s
                            """)
                    }
                }
            }
        }
    }
}
