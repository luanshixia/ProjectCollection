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
}

class AlbumDetailViewModel: ObservableObject {
    @Published var photos: [Photo] = []
    @Published var isShowingCamera = false
    @Published var selectedPhotos: Set<UUID> = []
    @Published var isMultiSelectMode = false
    
    let album: Album
    
    init(album: Album) {
        self.album = album
        // TODO: 从存储加载照片
        loadPhotos()
    }
    
    private func loadPhotos() {
        // 模拟数据
        photos = [
            Photo(fileName: "photo1.jpg", image: UIImage(systemName: "photo")),
            Photo(fileName: "photo2.jpg", image: UIImage(systemName: "photo")),
            Photo(fileName: "photo3.jpg", image: UIImage(systemName: "photo"))
        ]
    }
    
    func deleteSelectedPhotos() {
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
