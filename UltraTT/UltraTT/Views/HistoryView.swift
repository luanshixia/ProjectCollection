//
//  HistoryView.swift
//  UltraTT
//
//  Created by Yang Wang on 2/2/25.
//

import SwiftUI

struct HistoryView: View {
    var records: [QuizRecord]
    
    var body: some View {
        VStack {
            List {
                ForEach(records, id: \.id) { record in
                    HStack {
                        Label {
                            Text("\(record.question)")
                        } icon: {
                            Image(systemName: record.correctAnswer == record.userAnswer ? "checkmark.circle.fill" : "xmark.circle.fill")
                                .foregroundColor(record.correctAnswer == record.userAnswer ? .green : .red)
                        }
                        Spacer()
                        Text("\(record.timeSpent, specifier: "%.1f")s").font(.footnote)
                    }
                }
            }
        }
    }
}
