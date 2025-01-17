//
//  ArticleItemRow.swift
//  DevCheat
//
//  Created by Yang Wang on 1/15/25.
//

import SwiftUI

struct ArticleItemRow: View {
    var caption: String
    var items: [Article]

    var body: some View {
        VStack(alignment: .leading) {
            NavigationLink {
                ArticleListView(caption: caption, items: items)
            } label: {
                Text(caption + " >")
                    .font(.headline)
                    .padding(.leading, 15)
                    .padding(.top, 10)
            }

            ScrollView(.horizontal, showsIndicators: false) {
                HStack(alignment: .top, spacing: 0) {
                    ForEach(items.prefix(10)) { article in
                        NavigationLink {
                            ArticleView(article: article)
                        } label: {
                            ArticleListItem(article: article)
                        }
                    }
                }
                .padding(.leading, 10)
            }
            .padding(.bottom, 10)
        }
    }
}

#Preview {
    ArticleItemRow(caption: "Articles", items: ModelData().articles)
}
