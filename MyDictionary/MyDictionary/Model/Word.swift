//
//  Word.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/21/25.
//

import Foundation
import SwiftData

@Model
class Word {
    var text: String
    var dateAdded: Date
    var lastReviewed: Date?
    var nextReviewDate: Date?
    var confidence: Int
    var reviewCount: Int
    
    init(text: String) {
        self.text = text
        self.dateAdded = Date()
        self.confidence = 1
        self.reviewCount = 0
    }
    
    var formattedDateAdded: String {
        dateAdded.formatted(date: .abbreviated, time: .omitted)
    }
    
    var formattedNextReview: String {
        nextReviewDate?.formatted(date: .abbreviated, time: .omitted) ?? "Not scheduled"
    }
}
