//
//  AlbumManager.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import Foundation

class AlbumManager {
    static let shared = AlbumManager()
    private let fileManager = FileManager.default
    private let albumsFileName = "albums.json"
    
    @Published private(set) var albums: [Album] = []
    
    private init() {
        loadAlbums()
    }
    
    // MARK: - Album Operations
    
    func createAlbum(name: String) -> Album {
        let album = Album(name: name)
        albums.append(album)
        saveAlbums()
        return album
    }
    
    func deleteAlbum(_ album: Album) {
        albums.removeAll { $0.id == album.id }
        saveAlbums()
        // 删除相册对应的照片目录
        try? StorageManager.shared.deleteAlbumDirectory(albumId: album.id)
    }
    
    func updateAlbum(_ album: Album, newName: String) {
        if let index = albums.firstIndex(where: { $0.id == album.id }) {
            var updatedAlbum = album
            updatedAlbum.name = newName
            albums[index] = updatedAlbum
            saveAlbums()
        }
    }
    
    func getAlbum(by id: UUID) -> Album? {
        albums.first { $0.id == id }
    }
    
    // MARK: - Private Methods
    
    private func loadAlbums() {
        guard let url = try? getAlbumsFileURL() else { return }
        
        do {
            let data = try Data(contentsOf: url)
            albums = try JSONDecoder().decode([Album].self, from: data)
            print("已加载相册：\(albums.count)个")
        } catch {
            print("加载相册失败：\(error)")
            // 如果是首次启动，创建默认相册
            if albums.isEmpty {
                createDefaultAlbums()
            }
        }
    }
    
    private func saveAlbums() {
        guard let url = try? getAlbumsFileURL() else { return }
        
        do {
            let data = try JSONEncoder().encode(albums)
            try data.write(to: url, options: .atomic)
            print("保存相册成功：\(albums.count)个")
        } catch {
            print("保存相册失败：\(error)")
        }
    }
    
    private func getAlbumsFileURL() throws -> URL {
        try FileManager.default.url(
            for: .documentDirectory,
            in: .userDomainMask,
            appropriateFor: nil,
            create: true
        ).appendingPathComponent(albumsFileName)
    }
    
    private func createDefaultAlbums() {
        let defaultAlbums = [
            "学习资料",
            "文档记录",
            "参考信息",
            "临时记录"
        ]
        
        defaultAlbums.forEach { name in
            _ = createAlbum(name: name)
        }
    }
}
