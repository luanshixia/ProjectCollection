//
//  ViewHistory.swift
//  DevCheat
//
//  Created by Yang Wang on 1/22/25.
//

import Foundation
import SwiftData

@Model
class ViewHistory {
    var id: Int
    var viewedAt: Date
    
    init(id: Int, viewedAt: Date) {
        self.id = id
        self.viewedAt = viewedAt
    }
}
