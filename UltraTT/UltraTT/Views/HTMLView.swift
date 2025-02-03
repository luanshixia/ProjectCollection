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
    static let messageHandler = WebViewMessageHandler()
    
    func makeCoordinator() -> WebViewMessageHandler {
        return HTMLView.messageHandler
    }
    
    func makeUIView(context: Context) -> WKWebView {
        let configuration = WKWebViewConfiguration()
        configuration.userContentController
            .add(context.coordinator, name: "quizRecord")
        
        let webView = WKWebView(frame: .zero, configuration: configuration)
        webView.isOpaque = false
        webView.backgroundColor = UIColor.clear
        webView.scrollView.backgroundColor = UIColor.clear
        webView.configuration.preferences.setValue(true, forKey: "developerExtrasEnabled")
        webView.isInspectable = true
        return webView
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
