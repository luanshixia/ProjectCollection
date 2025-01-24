//
//  SearchView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/23/25.
//

import SwiftUI

struct SearchView: View {
    var model: ModelData
    @State var searchText: String = ""
    
    var body: some View {
        NavigationStack {
            List {
                ForEach(searchResults, id: \.self) { article in
                    NavigationLink {
                        ArticleView(article: article)
                    } label: {
                        Text(article.title)
                    }
                }
            }
            .navigationTitle("Search")
        }
        .searchable(text: $searchText)
    }
    
    var searchResults: [Article] {
        if searchText.isEmpty {
            return model.articles
        } else {
            return model.articles.filter { $0.title.localizedCaseInsensitiveContains(searchText) }
        }
    }
}

#Preview {
    SearchView(model: ModelData())
}
