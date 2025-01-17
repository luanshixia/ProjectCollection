//
//  HomeView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/16/25.
//

import SwiftUI

struct HomeView: View {
    var model: ModelData
    
    var body: some View {
        ScrollView {
            VStack(alignment: .leading) {
                ArticleItemRow(caption: "Featured", items: model.filterArticles(by: model.featured))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Web", items: model.filterArticles(by: model.web))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Command Line", items: model.filterArticles(by: model.commandLine))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "Others", items: model.filterArticles(by: model.others))
                    .listRowInsets(EdgeInsets())
                
                ArticleItemRow(caption: "A-Z", items: model.articles.sorted(by: { $0.title.uppercased() < $1.title.uppercased() }))
                    .listRowInsets(EdgeInsets())
            }
        }
        .navigationTitle("Home")
    }
}

#Preview {
    HomeView(model: ModelData())
}
