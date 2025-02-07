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
        updateAlbumsList()
    }
    
    func updateAlbumsList() {
        albums = albumManager.albumsWithPhotoCount()
    }
    
    func addAlbum() {
        guard !newAlbumName.trimmingCharacters(in: .whitespaces).isEmpty else { return }
        let album = albumManager.createAlbum(name: newAlbumName)
        updateAlbumsList()
        newAlbumName = ""
        isAddingNew = false
    }
    
    func deleteAlbum(at offsets: IndexSet) {
        offsets.forEach { index in
            let album = albums[index]
            albumManager.deleteAlbum(album)
        }
        updateAlbumsList()
    }
    
    func moveAlbum(from source: IndexSet, to destination: Int) {
        albums.move(fromOffsets: source, toOffset: destination)
        updateAlbumsList()
    }
    
    func renameAlbum(_ album: Album, to newName: String) {
        albumManager.updateAlbum(album, newName: newName)
        updateAlbumsList()
    }
    
    // 在返回到相册列表时更新照片数量
    func onAppear() {
        updateAlbumsList()
    }
}
