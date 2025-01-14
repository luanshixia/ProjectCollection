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
        .init(id: 1, title: "SwiftUI", url: "https://www.youtube.com/", description: "SwiftUI", publishedAt: Date(), category: .swift, isFavorite: true),
        .init(id: 2, title: "React", url: "https://www.youtube.com/", description: "SwiftUI", publishedAt: Date(), category: .web, isFavorite: false),
        .init(id: 3, title: "WinUI", url: "https://www.youtube.com/", description: "SwiftUI", publishedAt: Date(), category: .coding, isFavorite: false),
    ]
}
