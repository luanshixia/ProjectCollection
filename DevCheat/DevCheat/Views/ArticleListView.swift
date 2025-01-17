//
//  ArticleListView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/15/25.
//

import SwiftUI

struct ArticleListView: View {
    var caption: String
    var items: [Article]
    let adaptiveColumn = [
        GridItem(.adaptive(minimum: 160))
    ]
    
    var body: some View {
        ScrollView{
            LazyVGrid(columns: adaptiveColumn, spacing: 10) {
                ForEach(items) { article in
                    NavigationLink {
                        ArticleView(article: article)
                    } label: {
                        ArticleListItem(article: article, width: .infinity, height: 120)
                    }
                    .buttonStyle(PlainButtonStyle())
                }
            }
        }
        .navigationTitle(caption)
        .padding(.top, 10)
        .padding(.leading, 15)
        .padding(.trailing, 15)
    }
}

#Preview {
    ArticleListView(caption: "A-Z", items: ModelData().articles)
}
