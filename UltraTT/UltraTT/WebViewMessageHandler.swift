//
//  WebViewMessageHandler.swift
//  UltraTT
//
//  Created by Yang Wang on 2/2/25.
//

import SwiftUI
import WebKit

// Data structure for quiz records
struct QuizRecord: Codable, Identifiable {
    let id: UUID
    let timestamp: Date
    let question: String  // e.g., "7 × 8"
    let correctAnswer: Int
    let userAnswer: Int
    let numAttempts: Int
    let isCorrect: Bool
    let timeSpent: Double  // seconds
}

// Message handler to receive messages from JavaScript
class WebViewMessageHandler: NSObject, WKScriptMessageHandler {
    @AppStorage("quizRecords") private var storedRecords = Data()
    private var records: [QuizRecord] = []
    
    override init() {
        super.init()
        loadRecords()
    }
    
    private func loadRecords() {
        if let decoded = try? JSONDecoder().decode([QuizRecord].self, from: storedRecords) {
            records = decoded
        }
    }
    
    private func saveRecords() {
        if let encoded = try? JSONEncoder().encode(records) {
            storedRecords = encoded
        }
    }
    
    func userContentController(
        _ userContentController: WKUserContentController,
        didReceive message: WKScriptMessage
    ) {
        if message.name == "quizRecord",
           let dict = message.body as? [String: Any],
           let num1 = dict["num1"] as? Int,
           let num2 = dict["num2"] as? Int,
           let userAnswer = dict["userAnswer"] as? Int,
           let numAttempts = dict["numAttempts"] as? Int,
           let isCorrect = dict["isCorrect"] as? Bool,
           let timeSpent = dict["timeSpent"] as? Double {
            
            let record = QuizRecord(
                id: UUID(),
                timestamp: Date(),
                question: "\(num1) × \(num2)",
                correctAnswer: num1 * num2,
                userAnswer: userAnswer,
                numAttempts: numAttempts,
                isCorrect: isCorrect,
                timeSpent: timeSpent
            )
            
            records.append(record)
            saveRecords()
        }
    }
}
