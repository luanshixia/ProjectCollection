//
//  WordEntry.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import Foundation

struct WordEntry: Identifiable {
    let id = UUID()
    let word: String
    let dateAdded: Date
    var lastReviewed: Date?
    var reviewCount: Int
    var confidence: Int // 1-5 scale
}
