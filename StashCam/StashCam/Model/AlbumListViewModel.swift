//
//  AlbumListViewModel.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
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

class AlbumListViewModel: ObservableObject {
    @Published var albums: [Album] = []
    @Published var isAddingNew = false
    @Published var newAlbumName = ""
    
    private let albumManager = AlbumManager.shared
    
    init() {
        // 从 AlbumManager 获取相册列表
        albums = albumManager.albums
    }
    
    func addAlbum() {
        guard !newAlbumName.trimmingCharacters(in: .whitespaces).isEmpty else { return }
        let album = albumManager.createAlbum(name: newAlbumName)
        albums = albumManager.albums
        newAlbumName = ""
        isAddingNew = false
    }
    
    func deleteAlbum(at offsets: IndexSet) {
        offsets.forEach { index in
            let album = albums[index]
            albumManager.deleteAlbum(album)
        }
        albums = albumManager.albums
    }
    
    func moveAlbum(from source: IndexSet, to destination: Int) {
        albums.move(fromOffsets: source, toOffset: destination)
    }
    
    func renameAlbum(_ album: Album, to newName: String) {
        albumManager.updateAlbum(album, newName: newName)
        albums = albumManager.albums
    }
}
