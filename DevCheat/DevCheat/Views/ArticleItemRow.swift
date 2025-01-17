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
                Label(caption, systemImage: "chevron.right")
                    .labelStyle(CaptionLabelStyle())
                    .font(.headline)
                    .padding(.leading, 15)
                    .padding(.top, 10)
            }
            .buttonStyle(PlainButtonStyle())

            ScrollView(.horizontal, showsIndicators: false) {
                HStack(alignment: .top, spacing: 0) {
                    ForEach(items.prefix(10)) { article in
                        NavigationLink {
                            ArticleView(article: article)
                        } label: {
                            ArticleListItem(article: article)
                        }
                        .buttonStyle(PlainButtonStyle())
                    }
                }
                .padding(.leading, 10)
            }
            .padding(.bottom, 10)
        }
    }
}

struct CaptionLabelStyle: LabelStyle {
    func makeBody(configuration: Configuration) -> some View {
        HStack(alignment: .center, spacing: 3) {
            configuration.title
            configuration.icon.foregroundColor(.accentColor)
        }
    }
}

#Preview {
    ArticleItemRow(caption: "Articles", items: ModelData().articles)
}
