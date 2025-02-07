//
//  StorageManager.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import Foundation
import UIKit

class StorageManager {
    static let shared = StorageManager()
    private let fileManager = FileManager.default
    
    private init() {}
    
    // MARK: - Local Storage
    
    func savePhotoLocally(_ photo: Photo, in albumId: UUID) throws -> URL {
        let documentsDirectory = try getDocumentsDirectory()
        let albumDirectory = documentsDirectory.appendingPathComponent(albumId.uuidString)
        
        // 创建相册目录
        try? fileManager.createDirectory(at: albumDirectory,
                                         withIntermediateDirectories: true)
        
        // 保存图片文件
        let fileURL = albumDirectory.appendingPathComponent(photo.fileName)
        if let imageData = photo.image?.jpegData(compressionQuality: 0.8) {
            try imageData.write(to: fileURL)
        }
        
        return fileURL
    }
    
    func loadPhotosLocally(for albumId: UUID) throws -> [Photo] {
        let documentsDirectory = try getDocumentsDirectory()
        let albumDirectory = documentsDirectory.appendingPathComponent(albumId.uuidString)
        
        // 如果相册目录不存在，返回空数组
        guard fileManager.fileExists(atPath: albumDirectory.path) else {
            return []
        }
        
        // 获取目录中的所有文件
        let fileURLs = try fileManager.contentsOfDirectory(
            at: albumDirectory,
            includingPropertiesForKeys: [.creationDateKey],
            options: .skipsHiddenFiles
        )
        
        // 转换为 Photo 对象
        return fileURLs.compactMap { fileURL in
            guard let imageData = try? Data(contentsOf: fileURL),
                  let image = UIImage(data: imageData) else {
                return nil
            }
            
            return Photo(
                fileName: fileURL.lastPathComponent,
                image: image
            )
        }
    }
    
    func deletePhotoLocally(_ photo: Photo, in albumId: UUID) throws {
        let documentsDirectory = try getDocumentsDirectory()
        let fileURL = documentsDirectory
            .appendingPathComponent(albumId.uuidString)
            .appendingPathComponent(photo.fileName)
        
        if fileManager.fileExists(atPath: fileURL.path) {
            try fileManager.removeItem(at: fileURL)
        }
    }
    
    func getDocumentsDirectory() throws -> URL {
        try fileManager.url(
            for: .documentDirectory,
            in: .userDomainMask,
            appropriateFor: nil,
            create: true
        )
    }
}
