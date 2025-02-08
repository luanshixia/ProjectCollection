//
//  Album.swift
//  StashCam
//
//  Created by Yang Wang on 2/8/25.
//

import Foundation
import SwiftUI

struct Album: Identifiable, Codable, Hashable {
    let id: UUID
    var name: String
    var photoCount: Int
    var createdAt: Date
    
    init(id: UUID = UUID(), name: String, photoCount: Int = 0) {
        self.id = id
        self.name = name
        self.photoCount = photoCount
        self.createdAt = Date()
    }
}
