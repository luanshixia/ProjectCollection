//
//  HTMLView.swift
//  UltraTT
//
//  Created by Yang Wang on 2/1/25.
//

import SwiftUI
import WebKit

struct HTMLView: UIViewRepresentable {
    let htmlFileName: String
    
    func makeUIView(context: Context) -> WKWebView {
        return WKWebView()
    }
    
    func updateUIView(_ uiView: WKWebView, context: Context) {
        if let htmlPath = Bundle.main.path(forResource: htmlFileName, ofType: "html") {
            let url = URL(fileURLWithPath: htmlPath)
            let request = URLRequest(url: url)
            uiView.load(request)
        }
    }
}

#Preview {
    HTMLView(htmlFileName: "html")
        .edgesIgnoringSafeArea(.all)
}
