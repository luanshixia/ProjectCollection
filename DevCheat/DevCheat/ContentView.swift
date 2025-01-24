//
//  ContentView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/12/25.
//

import SwiftUI

struct ContentView: View {
    @Environment(ModelData.self) var model
    @State var selectedTab: Tabs = .home
    
    var body: some View {
        TabView(selection: $selectedTab) {
            Tab("Home", systemImage: "house", value: .home) {
                HomeView(model: model)
            }
            Tab("My", systemImage: "person", value: .my) {
                MyView(model: model)
            }
            Tab("Search", systemImage: "magnifyingglass", value: .search, role: .search) {
                SearchView(model: model)
            }
        }
    }
    
    enum Tabs: Equatable, Hashable {
        case home
        case my
        case search
    }
}

#Preview {
    ContentView()
        .environment(ModelData())
}
