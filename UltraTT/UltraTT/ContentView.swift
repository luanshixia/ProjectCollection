//
//  ContentView.swift
//  UltraTT
//
//  Created by Yang Wang on 2/1/25.
//

import SwiftUI

struct ContentView: View {
    @State var selectedTab: Tabs = .oneTo100
    @State var shouldPresentSheet: Bool = false
    
    var body: some View {
        NavigationStack {
            TabView(selection: $selectedTab) {
                Tab("1-100", systemImage: "house", value: .oneTo100) {
                    HTMLView(htmlFileName: "app1").background(.blue)
                }
                Tab("Products", systemImage: "person", value: .products) {
                    HTMLView(htmlFileName: "app2").background(.green)
                }
            }
            .accentColor(UIDevice.current.userInterfaceIdiom == .pad ? .black : .white)
            .toolbar {
                ToolbarItem(placement: .topBarTrailing) {
                    Button {
                        shouldPresentSheet.toggle()
                    } label: {
                        Image(systemName: "calendar")
                            .foregroundColor(.white)
                    }
                }
            }
            .sheet(isPresented: $shouldPresentSheet) {
                // on dismiss
            } content: {
                NavigationStack {
                    HistoryView()
                        .toolbar {
                            ToolbarItem(placement: .topBarTrailing) {
                                Button {
                                    shouldPresentSheet.toggle()
                                } label: {
                                    Text("Done")
                                }
                            }
                        }
                }
            }
        }
    }
    
    enum Tabs: Equatable, Hashable {
        case oneTo100
        case products
    }
}

#Preview {
    ContentView()
}
