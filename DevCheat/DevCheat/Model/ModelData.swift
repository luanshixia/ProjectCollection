//
//  ModelData.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import Foundation

@Observable
class ModelData {
    var articles: [Article] = [
        // generate some sample data
        .init(title: "SwiftUI", url: "https://www.youtube.com/", description: "SwiftUI", publishedAt: Date(), category: .swift, isFavorite: true),
        .init(title: "SwiftUI", url: "https://www.youtube.com/", description: "SwiftUI", publishedAt: Date(), category: .swift, isFavorite: false),
        .init(title: "SwiftUI", url: "https://www.youtube.com/", description: "SwiftUI", publishedAt: Date(), category: .swift, isFavorite: false),
    ]
}
