//
//  ContentView.swift
//  UltraTT
//
//  Created by Yang Wang on 2/1/25.
//

import SwiftUI

struct ContentView: View {
    @State var selectedTab: Tabs = .oneTo100
    
    var body: some View {
        TabView(selection: $selectedTab) {
            Tab("1-100", systemImage: "house", value: .oneTo100) {
                HTMLView(htmlFileName: "app1").background(.blue)
            }
            Tab("Products", systemImage: "person", value: .products) {
                HTMLView(htmlFileName: "app2").background(.green)
            }
        }
        .accentColor(UIDevice.current.userInterfaceIdiom == .pad ? .black : .white)
    }
    
    enum Tabs: Equatable, Hashable {
        case oneTo100
        case products
    }
}

#Preview {
    ContentView()
}
