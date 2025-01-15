//
//  ArticleView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import SwiftUI

struct ArticleView: View {
    var article: Article
    
    var body: some View {
        HTMLView(htmlFileName: article.fileName)
            .frame(maxWidth: .infinity, maxHeight: .infinity)
    }
}

#Preview {
    ArticleView(article: ModelData().articles[0])
}
