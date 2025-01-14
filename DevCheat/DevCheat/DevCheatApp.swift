//
//  DevCheatApp.swift
//  DevCheat
//
//  Created by Yang Wang on 1/12/25.
//

import SwiftUI

@main
struct DevCheatApp: App {
    @State private var model = ModelData()
    
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environment(model)
        }
    }
}
