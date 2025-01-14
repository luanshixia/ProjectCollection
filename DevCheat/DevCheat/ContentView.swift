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
        NavigationSplitView {
            List {
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
        } detail: {
            Text("Detail")
        }
    }
}

#Preview {
    ContentView()
        .environment(ModelData())
}
