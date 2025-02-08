//
//  Photo.swift
//  StashCam
//
//  Created by Yang Wang on 2/8/25.
//

import Foundation
import SwiftUI

struct Photo: Identifiable, Equatable {
    let id: UUID
    let fileName: String
    let createdAt: Date
    let image: UIImage?
    
    init(id: UUID = UUID(), fileName: String? = nil, image: UIImage?) {
        self.id = id
        self.createdAt = Date()
        
        // If fileName is not provided, generate one based on timestamp
        if let fileName = fileName {
            self.fileName = fileName
        } else {
            let dateFormatter = DateFormatter()
            dateFormatter.dateFormat = "yyyyMMdd_HHmmss_SSS"
            self.fileName = "IMG_\(dateFormatter.string(from: self.createdAt)).jpg"
        }
        
        self.image = image
    }
    
    static func == (lhs: Photo, rhs: Photo) -> Bool {
        lhs.id == rhs.id
    }
}
