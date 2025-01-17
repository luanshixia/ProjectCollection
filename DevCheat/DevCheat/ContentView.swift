//
//  ContentView.swift
//  DevCheat
//
//  Created by Yang Wang on 1/12/25.
//

import SwiftUI

struct ContentView: View {
    @Environment(ModelData.self) var model
    
    var body: some View {
        NavigationStack {
            HomeView(model: model)
        }
    }
}

#Preview {
    ContentView()
        .environment(ModelData())
}
