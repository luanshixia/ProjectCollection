//
//  ArticleListItem.swift
//  DevCheat
//
//  Created by Yang Wang on 1/15/25.
//

import SwiftUI

struct ArticleListItem: View {
    var article: Article
    var width: CGFloat? = 160
    var height: CGFloat? = 160
    
    static var styles: [Article.Category: LinearGradient] = [
        .mobile: LinearGradient(
            gradient: Gradient(colors: [.blue, .gray]),
            startPoint: .leading,
            endPoint: .trailing
        ),
        .web: LinearGradient(
            gradient: Gradient(colors: [.yellow, .green]),
            startPoint: .leading,
            endPoint: .trailing
        ),
        .devops: LinearGradient(
            gradient: Gradient(colors: [.blue, .purple]),
            startPoint: .leading,
            endPoint: .trailing
        ),
        .dataScience: LinearGradient(
            gradient: Gradient(colors: [.orange, .red]),
            startPoint: .leading,
            endPoint: .trailing
        ),
        .programming: LinearGradient(
            gradient: Gradient(colors: [.purple, .gray]),
            startPoint: .leading,
            endPoint: .trailing
        ),
        .utils: LinearGradient(
            gradient: Gradient(colors: [.green, .blue]),
            startPoint: .leading,
            endPoint: .trailing
        ),
    ]
    
    var body: some View {
        VStack(alignment: .leading) {
            RoundedRectangle(cornerRadius: 15)
                .fill(ArticleListItem.styles[article.category] ?? LinearGradient(
                    gradient: Gradient(colors: [.blue, .purple]),
                    startPoint: .leading,
                    endPoint: .trailing
                ))
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
