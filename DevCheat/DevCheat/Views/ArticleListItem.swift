//
//  ArticleListItem.swift
//  DevCheat
//
//  Created by Yang Wang on 1/15/25.
//

import SwiftUI

struct ArticleListItem: View {
    var article: Article
    var width: CGFloat = 160
    var height: CGFloat = 160
    
    var body: some View {
        VStack(alignment: .leading) {
            RoundedRectangle(cornerRadius: 15)
                .fill(
                    LinearGradient(
                        gradient: Gradient(colors: [.blue, .purple]),
                        startPoint: .leading,
                        endPoint: .trailing
                    )
                )
                .frame(width: width, height: height)
                .overlay(
                    Text(article.description)
                        .foregroundColor(.white)
                        .font(.title)
                        .bold()
                )
            Text(article.title)
                .foregroundStyle(.primary)
                .font(.caption)
        }
        .padding(.leading, 5)
        .padding(.trailing, 10)
    }
}

#Preview {
    ArticleListItem(article: ModelData().articles[0])
}
