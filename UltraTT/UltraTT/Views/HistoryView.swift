//
//  HistoryView.swift
//  UltraTT
//
//  Created by Yang Wang on 2/2/25.
//

import SwiftUI

struct DayRecord: Identifiable {
    let id: String // Date string
    let date: Date
    let records: [QuizRecord]
    
    var totalQuestions: Int { records.count }
    var correctAnswers: Int { records.filter { $0.isCorrect }.count }
    var totalTimeSpent: Double { records.reduce(0) { $0 + min($1.timeSpent, 300) } }
    var accuracy: Double { Double(correctAnswers) / Double(totalQuestions) * 100 }
}

// Individual record row view
struct QuizRecordRow: View {
    let record: QuizRecord
    let timeFormatter: DateComponentsFormatter
    
    var body: some View {
        HStack {
            Text(record.question)
                .font(.headline)
            
            Spacer()
            
            Text(record.timeSpent <= 300 ? timeFormatter.string(from: record.timeSpent) ?? "" : "> 5m")
                .font(.subheadline)
                .foregroundColor(.secondary)
            
            Image(systemName: record.isCorrect ? "checkmark.circle.fill" : "xmark.circle.fill")
                .foregroundColor(record.isCorrect ? .green : .red)
        }
        .padding(.vertical, 4)
    }
}

// Section header view
struct DayHeaderView: View {
    let dayRecord: DayRecord
    let dateString: String
    let timeFormatter: DateComponentsFormatter
    
    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text(dateString)
                .font(.headline)
            
            HStack {
                Label("\(dayRecord.totalQuestions)", systemImage: "list.number")
                Spacer()
                Label("\(dayRecord.accuracy, specifier: "%.1f")%", systemImage: "checkmark")
                Spacer()
                Label("\(timeFormatter.string(from: dayRecord.totalTimeSpent) ?? "")", systemImage: "clock")
            }
            .font(.footnote)
            .foregroundColor(.secondary)
        }
        .padding(.vertical, 8)
    }
}

struct HistoryView: View {
    @AppStorage("quizRecords") private var records: [QuizRecord] = []
    @State private var groupedRecords: [DayRecord] = []
    
    private let dateFormatter: DateFormatter = {
        let df = DateFormatter()
        df.dateStyle = .medium
        return df
    }()
    
    private let timeFormatter: DateComponentsFormatter = {
        let formatter = DateComponentsFormatter()
        formatter.allowedUnits = [.hour, .minute, .second]
        formatter.unitsStyle = .abbreviated
        return formatter
    }()
    
    private func loadAndGroupRecords() {
        let calendar = Calendar.current
        let grouped = Dictionary(grouping: records) { record in
            calendar.startOfDay(for: record.timestamp)
        }
        
        groupedRecords = grouped.map { date, records in
            DayRecord(
                id: dateFormatter.string(from: date),
                date: date,
                records: records.sorted { $0.timestamp > $1.timestamp }
            )
        }.sorted { $0.date > $1.date }
    }
    
    private func getRelativeDateString(_ date: Date) -> String {
        let calendar = Calendar.current
        if calendar.isDateInToday(date) {
            return "Today"
        } else if calendar.isDateInYesterday(date) {
            return "Yesterday"
        } else {
            return dateFormatter.string(from: date)
        }
    }
    
    var body: some View {
        List {
            ForEach(groupedRecords) { dayRecord in
                Section {
                    ForEach(dayRecord.records) { record in
                        QuizRecordRow(record: record, timeFormatter: timeFormatter)
                    }
                } header: {
                    DayHeaderView(
                        dayRecord: dayRecord,
                        dateString: getRelativeDateString(dayRecord.date),
                        timeFormatter: timeFormatter
                    )
                }
            }
        }
        .onAppear {
            loadAndGroupRecords()
        }
    }
}
