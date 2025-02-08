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

@MainActor
class AlbumDetailViewModel: ObservableObject {
    // MARK: - Published Properties
    
    @Published var photos: [Photo] = []
    @Published var selectedPhotos: Set<UUID> = []
    @Published var isMultiSelectMode = false
    @Published var isShowingCamera = false
    @Published var selectedPhoto: Photo?
    @Published var isLoading = false
    @Published var error: Error?
    
    // MARK: - Properties
    
    let album: Album
    private let storageManager = StorageManager.shared
    
    // MARK: - Initialization
    
    init(album: Album) {
        self.album = album
        loadPhotos()
    }
    
    // MARK: - Public Methods
    
    func loadPhotos() {
        Task {
            isLoading = true
            defer { isLoading = false }
            
            do {
                photos = try await loadLocalPhotos()
            } catch {
                self.error = error
            }
        }
    }
    
    func savePhoto(_ photo: Photo) {
        Task {
            do {
                try await savePhotoLocally(photo)
                await MainActor.run {
                    photos.append(photo)
                }
            } catch {
                await MainActor.run {
                    self.error = error
                }
            }
        }
    }
    
    func deleteSelectedPhotos() {
        Task {
            do {
                for photoId in selectedPhotos {
                    guard let photo = photos.first(where: { $0.id == photoId }) else { continue }
                    try await deletePhotoLocally(photo)
                }
                
                await MainActor.run {
                    photos.removeAll { selectedPhotos.contains($0.id) }
                    selectedPhotos.removeAll()
                    isMultiSelectMode = false
                }
            } catch {
                await MainActor.run {
                    self.error = error
                }
            }
        }
    }
    
    func togglePhotoSelection(_ photo: Photo) {
        if selectedPhotos.contains(photo.id) {
            selectedPhotos.remove(photo.id)
        } else {
            selectedPhotos.insert(photo.id)
        }
    }
    
    // MARK: - Private Storage Methods
    
    private func loadLocalPhotos() async throws -> [Photo] {
        try await withCheckedThrowingContinuation { continuation in
            do {
                let photos = try storageManager.loadPhotosLocally(for: album.id)
                continuation.resume(returning: photos)
            } catch {
                continuation.resume(throwing: error)
            }
        }
    }
    
    private func savePhotoLocally(_ photo: Photo) async throws {
        try await withCheckedThrowingContinuation { continuation in
            do {
                _ = try storageManager.savePhotoLocally(photo, in: album.id)
                continuation.resume()
            } catch {
                continuation.resume(throwing: error)
            }
        }
    }
    
    private func deletePhotoLocally(_ photo: Photo) async throws {
        try await withCheckedThrowingContinuation { continuation in
            do {
                try storageManager.deletePhotoLocally(photo, in: album.id)
                continuation.resume()
            } catch {
                continuation.resume(throwing: error)
            }
        }
    }
}

// MARK: - Error Types

extension AlbumDetailViewModel {
    enum PhotoError: LocalizedError {
        case saveFailed
        case deleteFailed
        case loadFailed
        
        var errorDescription: String? {
            switch self {
            case .saveFailed:
                return "保存照片失败"
            case .deleteFailed:
                return "删除照片失败"
            case .loadFailed:
                return "加载照片失败"
            }
        }
    }
}
