//
//  ContentView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI

struct ContentView: View {
    @State private var selectedTab = 0
    
    var body: some View {
        TabView(selection: $selectedTab) {
            LookupView()
                .tabItem {
                    Label("Look Up", systemImage: "book")
                }
                .tag(0)
            
            ReviewView()
                .tabItem {
                    Label("Review", systemImage: "square.stack")
                }
                .tag(1)
            
            StatsView()
                .tabItem {
                    Label("Stats", systemImage: "chart.bar")
                }
                .tag(2)
        }
    }
}
