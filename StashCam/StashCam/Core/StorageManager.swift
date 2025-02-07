//
//  StorageManager.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import Foundation
import UIKit
import CloudKit

class StorageManager {
    static let shared = StorageManager()
    
    private let fileManager = FileManager.default
    private let container = CKContainer.default()
    private let database: CKDatabase
    
    private init() {
        database = container.privateCloudDatabase
    }
    
    // MARK: - Local Storage
    
    func savePhotoLocally(_ photo: Photo, in albumId: UUID) throws -> URL {
        let documentsDirectory = try getDocumentsDirectory()
        let albumDirectory = documentsDirectory.appendingPathComponent(albumId.uuidString)
        
        try? fileManager.createDirectory(at: albumDirectory,
                                         withIntermediateDirectories: true)
        
        let fileURL = albumDirectory.appendingPathComponent(photo.fileName)
        if let imageData = photo.image?.jpegData(compressionQuality: 0.8) {
            try imageData.write(to: fileURL)
        }
        
        return fileURL
    }
    
    func loadPhotoLocally(fileName: String, albumId: UUID) -> UIImage? {
        guard let documentsDirectory = try? getDocumentsDirectory() else { return nil }
        let fileURL = documentsDirectory
            .appendingPathComponent(albumId.uuidString)
            .appendingPathComponent(fileName)
        
        guard let imageData = try? Data(contentsOf: fileURL) else { return nil }
        return UIImage(data: imageData)
    }
    
    func deletePhotoLocally(_ photo: Photo, in albumId: UUID) throws {
        let documentsDirectory = try getDocumentsDirectory()
        let fileURL = documentsDirectory
            .appendingPathComponent(albumId.uuidString)
            .appendingPathComponent(photo.fileName)
        
        try fileManager.removeItem(at: fileURL)
    }
    
    func getDocumentsDirectory() throws -> URL {
        try fileManager.url(for: .documentDirectory,
                            in: .userDomainMask,
                            appropriateFor: nil,
                            create: true)
    }
    
    // MARK: - iCloud Sync
    
    func syncPhotoToCloud(_ photo: Photo, in albumId: UUID) {
        let record = CKRecord(recordType: "Photo")
        record.setValue(photo.fileName, forKey: "fileName")
        record.setValue(albumId.uuidString, forKey: "albumId")
        
        if let imageData = photo.image?.jpegData(compressionQuality: 0.8) {
            let imageAsset = CKAsset(fileURL: saveTemporaryImage(imageData))
            record.setValue(imageAsset, forKey: "imageData")
        }
        
        database.save(record) { record, error in
            if let error = error {
                print("Error saving to iCloud: \(error.localizedDescription)")
            }
        }
    }
    
    func fetchPhotosFromCloud(albumId: UUID, completion: @escaping ([Photo]) -> Void) {
        let predicate = NSPredicate(format: "albumId == %@", albumId.uuidString)
        let query = CKQuery(recordType: "Photo", predicate: predicate)
        
        database.perform(query, inZoneWith: nil) { records, error in
            guard let records = records, error == nil else {
                completion([])
                return
            }
            
            let photos = records.compactMap { record -> Photo? in
                guard let fileName = record["fileName"] as? String,
                      let imageAsset = record["imageData"] as? CKAsset,
                      let imageData = try? Data(contentsOf: imageAsset.fileURL!),
                      let image = UIImage(data: imageData) else {
                    return nil
                }
                return Photo(fileName: fileName, image: image)
            }
            
            DispatchQueue.main.async {
                completion(photos)
            }
        }
    }
    
    func deletePhotoFromCloud(_ photo: Photo, in albumId: UUID) {
        let predicate = NSPredicate(format: "fileName == %@ AND albumId == %@",
                                    photo.fileName, albumId.uuidString)
        let query = CKQuery(recordType: "Photo", predicate: predicate)
        
        database.perform(query, inZoneWith: nil) { records, error in
            guard let recordID = records?.first?.recordID else { return }
            
            self.database.delete(withRecordID: recordID) { _, error in
                if let error = error {
                    print("Error deleting from iCloud: \(error.localizedDescription)")
                }
            }
        }
    }
    
    private func saveTemporaryImage(_ imageData: Data) -> URL {
        let temporaryDirectory = FileManager.default.temporaryDirectory
        let fileURL = temporaryDirectory.appendingPathComponent(UUID().uuidString + ".jpg")
        try? imageData.write(to: fileURL)
        return fileURL
    }
}
