//
//  ArticleView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import SwiftUI

struct ArticleView: View {
    var article: Article
    @AppStorage("viewHistory") var viewHistory: [Int: Date] = [:]
    @AppStorage("favorites") var favorites: [Int: Date] = [:]
    
    var body: some View {
        HTMLView(htmlFileName: article.fileName)
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .onAppear {
                viewHistory.updateValue(Date(), forKey: article.id)
            }
            .toolbar {
                ToolbarItem(placement: .topBarTrailing) {
                    Button {
                        if favorites[article.id] != nil {
                            favorites[article.id] = nil
                        } else {
                            favorites[article.id] = Date()
                        }
                    } label: {
                        Image(systemName: favorites[article.id] != nil ? "star.fill" : "star")
                            .foregroundColor(.accentColor)
                    }
                }
            }
    }
}

#Preview {
    ArticleView(article: ModelData().articles[0])
}
