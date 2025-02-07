//
//  AlbumDetailViewModel.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import Foundation
import SwiftUI

struct Photo: Identifiable, Equatable {
    let id: UUID
    let fileName: String
    let createdAt: Date
    var image: UIImage?
    
    init(id: UUID = UUID(), fileName: String, image: UIImage? = nil) {
        self.id = id
        self.fileName = fileName
        self.createdAt = Date()
        self.image = image
    }
    
    var localURL: URL? {
        guard let documentsDirectory = try? StorageManager.shared.getDocumentsDirectory() else {
            return nil
        }
        return documentsDirectory
            .appendingPathComponent(id.uuidString)
            .appendingPathComponent(fileName)
    }
}

class AlbumDetailViewModel: ObservableObject {
    @Published var photos: [Photo] = []
    @Published var isShowingCamera = false
    @Published var selectedPhotos: Set<UUID> = []
    @Published var isMultiSelectMode = false
    @Published var isLoading = false
    @Published var error: Error?
    @Published var selectedPhoto: Photo?
    
    let album: Album
    private let storageManager = StorageManager.shared
    
    init(album: Album) {
        self.album = album
        loadPhotos()
    }
    
    func loadPhotos() {
        isLoading = true
        
        storageManager.fetchPhotosFromCloud(albumId: album.id) { [weak self] photos in
            self?.photos = photos
            self?.isLoading = false
        }
    }
    
    func savePhoto(_ photo: Photo) {
        do {
            try storageManager.savePhotoLocally(photo, in: album.id)
            storageManager.syncPhotoToCloud(photo, in: album.id)
            photos.append(photo)
        } catch {
            self.error = error
        }
    }
    
    func deleteSelectedPhotos() {
        for photoId in selectedPhotos {
            guard let photo = photos.first(where: { $0.id == photoId }) else { continue }
            
            try? storageManager.deletePhotoLocally(photo, in: album.id)
            storageManager.deletePhotoFromCloud(photo, in: album.id)
        }
        
        // update view
        photos.removeAll { selectedPhotos.contains($0.id) }
        selectedPhotos.removeAll()
        isMultiSelectMode = false
    }
    
    func togglePhotoSelection(_ photo: Photo) {
        if selectedPhotos.contains(photo.id) {
            selectedPhotos.remove(photo.id)
        } else {
            selectedPhotos.insert(photo.id)
        }
    }
}
