//
//  AlbumListViewModel.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import Foundation
import SwiftUI

struct Album: Identifiable, Hashable {
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
    
    init() {
        // 初始化默认相册
        albums = [
            Album(name: "学习资料"),
            Album(name: "文档记录"),
            Album(name: "参考信息"),
            Album(name: "临时记录")
        ]
    }
    
    func addAlbum() {
        guard !newAlbumName.trimmingCharacters(in: .whitespaces).isEmpty else { return }
        let album = Album(name: newAlbumName)
        albums.append(album)
        newAlbumName = ""
        isAddingNew = false
    }
    
    func deleteAlbum(at offsets: IndexSet) {
        albums.remove(atOffsets: offsets)
    }
    
    func moveAlbum(from source: IndexSet, to destination: Int) {
        albums.move(fromOffsets: source, toOffset: destination)
    }
}
