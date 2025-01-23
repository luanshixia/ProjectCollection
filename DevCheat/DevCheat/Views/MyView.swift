//
//  MyView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/23/25.
//

import SwiftData
import SwiftUI

struct MyView: View {
    var model: ModelData
    @AppStorage("viewHistory") var viewHistory: [Int: Date] = [:]
    @AppStorage("favorites") var favorites: [Int: Date] = [:]
    
    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(alignment: .leading) {
                    ArticleItemRow(caption: "Recent", items: model.selectArticles(getRecent(viewHistory)))
                        .listRowInsets(EdgeInsets())
                    
                    ArticleItemRow(caption: "Favorites", items: model.selectArticles(getRecent(favorites)))
                        .listRowInsets(EdgeInsets())
                }
            }
            .navigationTitle("My")
        }
    }
                               
    func getRecent(_ dict: [Int: Date]) -> [Int] {
        Array(dict.keys.sorted { dict[$0]! > dict[$1]! })
    }
}

#Preview {
    MyView(model: ModelData())
}
