//
//  WordSuggestionsManager.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/21/25.
//

import Foundation

@Observable class WordSuggestionsManager {
    static let shared = WordSuggestionsManager()
    private var commonWords: Set<String>
    
    init() {
        // Initialize with a set of common English words
        // This could be loaded from a file in a real app
        commonWords = Set([
            "able", "about", "absolute", "accept", "account", "achieve", "across", "active", "actual", "adapt",
            "better", "between", "beyond", "board", "build", "business", "busy", "calculate", "camera", "campaign",
            "determine", "develop", "different", "difficult", "digital", "direct", "direction", "discover", "discuss",
            "effect", "effort", "eight", "either", "electric", "eleven", "else", "employ", "encourage", "engine",
            "familiar", "family", "famous", "father", "favor", "feature", "federal", "feeling", "fifteen", "figure",
            "great", "green", "ground", "group", "grow", "growth", "guess", "guide", "happen", "happy",
            "identify", "ignore", "image", "imagine", "impact", "improve", "include", "increase", "indicate",
            "language", "large", "last", "late", "later", "laugh", "launch", "learn", "least", "leave",
            "machine", "magazine", "maintain", "major", "majority", "make", "manage", "management", "manager",
            "native", "natural", "nature", "near", "nearly", "necessary", "need", "negative", "neither", "network",
            "observe", "obtain", "obvious", "occur", "offer", "office", "officer", "official", "often", "operate",
            "quality", "quarter", "question", "quick", "quickly", "quiet", "quite", "quote", "radio", "raise",
            "science", "scientific", "scientist", "score", "screen", "search", "season", "second", "secret",
            "understand", "unique", "unit", "unite", "universe", "university", "unless", "until", "unusual", "update",
            "vacation", "valid", "value", "variable", "variety", "various", "version", "very", "video", "view",
            "watch", "water", "weather", "website", "wedding", "week", "weekend", "welcome", "world", "write"
        ])
    }
    
    func getSuggestions(for text: String) -> [String] {
        guard !text.isEmpty else { return [] }
        
        let lowercaseText = text.lowercased()
        return commonWords
            .filter { $0.hasPrefix(lowercaseText) }
            .sorted()
            .prefix(5)
            .map { $0 }
    }
}
