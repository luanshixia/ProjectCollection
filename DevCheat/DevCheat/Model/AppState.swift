//
//  AppState.swift
//  DevCheat
//
//  Created by Yang Wang on 1/21/25.
//

import Foundation
import SwiftData

@Model
class AppState {
    var viewHistory: Dictionary<Int, Date>
    
    init(viewHistory: Dictionary<Int, Date>) {
        self.viewHistory = viewHistory
    }
    
    func recordView(_ id: Int) {
        viewHistory[id] = Date()
    }
    
    func getRecent() -> [Int] {
        Array(viewHistory.keys.sorted { viewHistory[$0]! > viewHistory[$1]! }.prefix(10))
    }
}
