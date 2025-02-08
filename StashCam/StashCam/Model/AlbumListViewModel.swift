//
//  AlbumListViewModel.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import Foundation
import SwiftUI

class AlbumListViewModel: ObservableObject {
    @Published var albums: [Album] = []
    @Published var isAddingNew = false
    @Published var newAlbumName = ""
    @Published var showDeletionAlert = false
    @Published var albumToDelete: Album?
    
    private let albumManager = AlbumManager.shared
    
    init() {
        updateAlbumsList()
    }
    
    func updateAlbumsList() {
        albums = albumManager.albumsWithPhotoCount()
    }
    
    func handleDeleteAlbum(at offsets: IndexSet) {
        guard let index = offsets.first else { return }
        let album = albums[index]
        if album.photoCount > 0 {
            albumToDelete = album
            showDeletionAlert = true
        } else {
            albumManager.deleteAlbum(album)
            updateAlbumsList()
        }
    }
    
    func addAlbum() {
        guard !newAlbumName.trimmingCharacters(in: .whitespaces).isEmpty else { return }
        let _ = albumManager.createAlbum(name: newAlbumName)
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
    
    func onAppear() {
        updateAlbumsList()
    }
}
