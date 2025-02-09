//
//  LocalizedString.swift
//  StashCam
//
//  Created by Yang Wang on 2/8/25.
//

import SwiftUI

struct LocalizedString {
    static let appName = String(localized: "app.name")
    static let deleteWarning = String(localized: "album.delete.warning")
    static let buttonCancel = String(localized: "button.cancel")
    static let buttonDone = String(localized: "button.done")
    
    static func deleteMessage(albumName: String, photoCount: Int) -> String {
        String(format: String(localized: "album.delete.message"),
               albumName, photoCount)
    }
    
    static func photoCount(_ count: Int) -> String {
        String(format: String(localized: "album.photos.count"),
               count)
    }
}
