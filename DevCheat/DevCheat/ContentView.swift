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
                ArticleItemRow(caption: "Featured", items: model.filterArticles(by: model.featured))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Web", items: model.filterArticles(by: model.web))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Command Line", items: model.filterArticles(by: model.commandLine))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Others", items: model.filterArticles(by: model.others))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "A-Z", items: model.articles)
                    .listRowInsets(EdgeInsets())
                
//                ForEach(model.articles) { article in
//                    NavigationLink {
//                        ArticleView(article: article)
//                    } label: {
//                        Text(article.title)
//                    }
//                }
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
