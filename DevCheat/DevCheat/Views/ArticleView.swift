//
//  ArticleView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import SwiftUI

struct ArticleView: View {
    var article: Article
    @AppStorage("viewHistory") var viewHistory: [Int: Date] = [:]
    
    var body: some View {
        HTMLView(htmlFileName: article.fileName)
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .onAppear {
                viewHistory.updateValue(Date(), forKey: article.id)
            }
    }
}

#Preview {
    ArticleView(article: ModelData().articles[0])
}
