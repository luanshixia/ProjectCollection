//
//  AlbumListView.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import SwiftUI

struct AlbumListView: View {
    @StateObject private var viewModel = AlbumListViewModel()
    @State private var editMode: EditMode = .inactive
    @State private var selectedAlbum: Album?
    
    var body: some View {
        NavigationStack {
            List {
                ForEach(viewModel.albums) { album in
                    NavigationLink(value: album) {
                        AlbumRow(album: album)
                    }
                }
                .onDelete(perform: viewModel.handleDeleteAlbum)
                .onMove(perform: viewModel.moveAlbum)
            }
            .navigationTitle(LocalizedString.appName)
            .navigationDestination(for: Album.self) { album in
                AlbumDetailView(album: album)
            }
            .toolbar {
                ToolbarItem(placement: .navigationBarLeading) {
                    EditButton()
                }
                
                ToolbarItem(placement: .navigationBarTrailing) {
                    Button {
                        viewModel.isAddingNew = true
                    } label: {
                        Image(systemName: "plus")
                    }
                }
            }
            .alert(LocalizedString.deleteWarning, isPresented: $viewModel.showDeletionAlert, presenting: viewModel.albumToDelete) { album in
                Button(LocalizedString.buttonCancel, role: .cancel) {}
            } message: { album in
                Text(LocalizedString.deleteMessage(albumName: album.name, photoCount: album.photoCount))
            }
            .sheet(isPresented: $viewModel.isAddingNew) {
                NavigationStack {
                    AddAlbumView(viewModel: viewModel)
                }
            }
            .environment(\.editMode, $editMode)
            .onAppear {
                viewModel.onAppear()
            }
        }
    }
}

struct AlbumRow: View {
    let album: Album
    
    var body: some View {
        HStack {
            Image(systemName: "folder.fill")
                .foregroundColor(.blue)
                .font(.title2)
            
            VStack(alignment: .leading) {
                Text(album.name)
                    .font(.headline)
                Text(LocalizedString.photoCount(album.photoCount))
                    .font(.caption)
                    .foregroundColor(.secondary)
            }
            
            Spacer()
            
            Image(systemName: "camera.fill")
                .foregroundColor(.blue)
                .font(.title3)
        }
        .padding(.vertical, 8)
    }
}

struct AddAlbumView: View {
    @ObservedObject var viewModel: AlbumListViewModel
    @Environment(\.dismiss) private var dismiss
    
    var body: some View {
        Form {
            Section {
                TextField("相册名称", text: $viewModel.newAlbumName)
            }
        }
        .navigationTitle("新建相册")
        .navigationBarTitleDisplayMode(.inline)
        .toolbar {
            ToolbarItem(placement: .navigationBarLeading) {
                Button("取消") {
                    viewModel.newAlbumName = ""
                    dismiss()
                }
            }
            
            ToolbarItem(placement: .navigationBarTrailing) {
                Button("完成") {
                    viewModel.addAlbum()
                    dismiss()
                }
                .disabled(viewModel.newAlbumName.trimmingCharacters(in: .whitespaces).isEmpty)
            }
        }
    }
}
