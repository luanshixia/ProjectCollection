//
//  ArticleView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/13/25.
//

import SwiftUI

struct ArticleView: View {
    var article: Article?
    
    var body: some View {
        Text(article?.title ?? "")
    }
}

#Preview {
    ArticleView(article: nil)
}
