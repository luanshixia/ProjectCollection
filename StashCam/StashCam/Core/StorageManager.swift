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
        if !fileManager.fileExists(atPath: albumDirectory.path) {
            try fileManager.createDirectory(at: albumDirectory,
                                            withIntermediateDirectories: true,
                                            attributes: nil)
        }
        
        // 保存图片文件
        let fileURL = albumDirectory.appendingPathComponent(photo.fileName)
        if let imageData = photo.image?.jpegData(compressionQuality: 0.8) {
            try imageData.write(to: fileURL, options: .atomic)
            print("保存照片到: \(fileURL.path)") // Debug 日志
        } else {
            throw StorageError.invalidImageData
        }
        
        return fileURL
    }
    
    func loadPhotosLocally(for albumId: UUID) throws -> [Photo] {
        let documentsDirectory = try getDocumentsDirectory()
        let albumDirectory = documentsDirectory.appendingPathComponent(albumId.uuidString)
        
        print("加载相册目录: \(albumDirectory.path)") // Debug 日志
        
        // 如果相册目录不存在，创建它
        if !fileManager.fileExists(atPath: albumDirectory.path) {
            try fileManager.createDirectory(at: albumDirectory,
                                            withIntermediateDirectories: true,
                                            attributes: nil)
            return []
        }
        
        // 获取目录中的所有文件
        let fileURLs = try fileManager.contentsOfDirectory(
            at: albumDirectory,
            includingPropertiesForKeys: [.creationDateKey],
            options: .skipsHiddenFiles
        )
        
        print("发现文件数量: \(fileURLs.count)") // Debug 日志
        
        // 转换为 Photo 对象
        var photos: [Photo] = []
        for fileURL in fileURLs {
            do {
                let imageData = try Data(contentsOf: fileURL)
                if let image = UIImage(data: imageData) {
                    let photo = Photo(
                        fileName: fileURL.lastPathComponent,
                        image: image
                    )
                    photos.append(photo)
                    print("成功加载照片: \(fileURL.lastPathComponent)") // Debug 日志
                }
            } catch {
                print("加载照片失败: \(fileURL.lastPathComponent), 错误: \(error)") // Debug 日志
                continue
            }
        }
        
        // 按创建时间排序
        return photos.sorted { $0.createdAt > $1.createdAt }
    }
    
    func deletePhotoLocally(_ photo: Photo, in albumId: UUID) throws {
        let documentsDirectory = try getDocumentsDirectory()
        let fileURL = documentsDirectory
            .appendingPathComponent(albumId.uuidString)
            .appendingPathComponent(photo.fileName)
        
        if fileManager.fileExists(atPath: fileURL.path) {
            try fileManager.removeItem(at: fileURL)
            print("删除照片: \(fileURL.path)") // Debug 日志
        }
    }
    
    func getDocumentsDirectory() throws -> URL {
        guard let documentsDirectory = fileManager.urls(
            for: .documentDirectory,
            in: .userDomainMask
        ).first else {
            throw StorageError.documentsDirectoryNotFound
        }
        return documentsDirectory
    }
    
    func deleteAlbumDirectory(albumId: UUID) throws {
        let documentsDirectory = try getDocumentsDirectory()
        let albumDirectory = documentsDirectory.appendingPathComponent(albumId.uuidString)
        
        if fileManager.fileExists(atPath: albumDirectory.path) {
            try fileManager.removeItem(at: albumDirectory)
        }
    }
}

// MARK: - Error Types

extension StorageManager {
    enum StorageError: LocalizedError {
        case documentsDirectoryNotFound
        case invalidImageData
        case fileOperationFailed
        
        var errorDescription: String? {
            switch self {
            case .documentsDirectoryNotFound:
                return "无法访问文档目录"
            case .invalidImageData:
                return "无效的图片数据"
            case .fileOperationFailed:
                return "文件操作失败"
            }
        }
    }
}
