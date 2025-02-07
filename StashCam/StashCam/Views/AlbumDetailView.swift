//
//  AlbumDetailView.swift
//  StashCam
//
//  Created by Yang Wang on 2/6/25.
//

import SwiftUI

struct AlbumDetailView: View {
    @StateObject private var viewModel: AlbumDetailViewModel
    @Environment(\.dismiss) private var dismiss
    
    private let columns = [
        GridItem(.flexible(), spacing: 1),
        GridItem(.flexible(), spacing: 1),
        GridItem(.flexible(), spacing: 1)
    ]
    
    init(album: Album) {
        _viewModel = StateObject(wrappedValue: AlbumDetailViewModel(album: album))
    }
    
    var body: some View {
        ZStack {
            photosGrid
                .navigationTitle(viewModel.album.name)
                .navigationBarTitleDisplayMode(.inline)
                .toolbar {
                    leadingToolbarItems
                    trailingToolbarItems
                }
            
            if viewModel.isLoading {
                ProgressView()
                    .scaleEffect(1.5)
                    .frame(maxWidth: .infinity, maxHeight: .infinity)
                    .background(Color.black.opacity(0.2))
            }
            
            if viewModel.photos.isEmpty && !viewModel.isLoading {
                emptyStateView
            }
        }
        .sheet(isPresented: $viewModel.isShowingCamera) {
            CameraView(
                isPresented: $viewModel.isShowingCamera,
                albumId: viewModel.album.id
            ) { photo in
                viewModel.savePhoto(photo)
            }
        }
        .alert("错误", isPresented: .constant(viewModel.error != nil)) {
            Button("确定") {
                viewModel.error = nil
            }
        } message: {
            if let error = viewModel.error {
                Text(error.localizedDescription)
            }
        }
    }
    
    private var photosGrid: some View {
        ScrollView {
            LazyVGrid(columns: columns, spacing: 1) {
                ForEach(viewModel.photos) { photo in
                    PhotoGridItem(
                        photo: photo,
                        isSelected: viewModel.selectedPhotos.contains(photo.id),
                        isMultiSelectMode: viewModel.isMultiSelectMode
                    )
                    .onTapGesture {
                        if viewModel.isMultiSelectMode {
                            viewModel.togglePhotoSelection(photo)
                        } else {
                            viewModel.selectedPhoto = photo
                        }
                    }
                    .onLongPressGesture {
                        viewModel.isMultiSelectMode = true
                        viewModel.togglePhotoSelection(photo)
                    }
                }
            }
        }
        .fullScreenCover(item: $viewModel.selectedPhoto) { photo in
            PhotoDetailView(photo: photo)
        }
    }
    
    private var emptyStateView: some View {
        VStack(spacing: 16) {
            Image(systemName: "photo.on.rectangle.angled")
                .font(.system(size: 48))
                .foregroundColor(.gray)
            
            Text("没有照片")
                .font(.headline)
                .foregroundColor(.gray)
            
            Button {
                viewModel.isShowingCamera = true
            } label: {
                Text("拍摄照片")
                    .font(.headline)
                    .foregroundColor(.white)
                    .padding(.horizontal, 20)
                    .padding(.vertical, 10)
                    .background(Color.blue)
                    .cornerRadius(8)
            }
        }
    }
    
    private var leadingToolbarItems: some ToolbarContent {
        ToolbarItemGroup(placement: .navigationBarLeading) {
            if viewModel.isMultiSelectMode {
                Button("取消") {
                    viewModel.isMultiSelectMode = false
                    viewModel.selectedPhotos.removeAll()
                }
            }
        }
    }
    
    private var trailingToolbarItems: some ToolbarContent {
        ToolbarItemGroup(placement: .navigationBarTrailing) {
            if viewModel.isMultiSelectMode {
                Button {
                    viewModel.deleteSelectedPhotos()
                } label: {
                    Image(systemName: "trash")
                        .foregroundColor(.red)
                }
                .disabled(viewModel.selectedPhotos.isEmpty)
            } else {
                Button {
                    viewModel.isShowingCamera = true
                } label: {
                    Image(systemName: "camera")
                }
            }
        }
    }
}

// PhotoGridItem.swift
struct PhotoGridItem: View {
    let photo: Photo
    let isSelected: Bool
    let isMultiSelectMode: Bool
    
    var body: some View {
        ZStack(alignment: .topTrailing) {
            if let image = photo.image {
                Image(uiImage: image)
                    .resizable()
                    .aspectRatio(1, contentMode: .fill)
                    .clipped()
                    .overlay(isMultiSelectMode ? Color.black.opacity(isSelected ? 0.3 : 0) : Color.clear)
            }
            
            if isMultiSelectMode {
                ZStack {
                    Circle()
                        .fill(isSelected ? Color.blue : Color.white)
                        .frame(width: 24, height: 24)
                    
                    if isSelected {
                        Image(systemName: "checkmark")
                            .font(.caption)
                            .foregroundColor(.white)
                    }
                }
                .padding(4)
            }
        }
    }
}

// Preview Provider
struct AlbumDetailView_Previews: PreviewProvider {
    static var previews: some View {
        NavigationStack {
            AlbumDetailView(album: Album(name: "测试相册"))
        }
    }
}
