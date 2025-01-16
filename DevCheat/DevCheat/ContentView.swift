//
//  ContentView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/12/25.
//

import SwiftUI

struct ContentView: View {
    @Environment(ModelData.self) var model
    
    var body: some View {
        NavigationStack {
            List {
                ArticleItemRow(caption: "Featured", items: model.articles)
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Web", items: model.articles)
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Command Line", items: model.articles)
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Python", items: model.articles)
                    .listRowInsets(EdgeInsets())
                
                ForEach(model.articles) { article in
                    NavigationLink {
                        ArticleView(article: article)
                    } label: {
                        Text(article.title)
                    }
                }
            }
            .listStyle(.inset)
            .navigationTitle("Home")
        }
    }
}

#Preview {
    ContentView()
        .environment(ModelData())
}
