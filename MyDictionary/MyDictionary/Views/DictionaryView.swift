//
//  DictionaryView.swift
//  MyDictionary
//
//  Created by Yang Wang on 2/20/25.
//

import SwiftUI
import UIKit

struct DictionaryView: View {
    @State private var word: String = ""
    @State private var showDefinition: Bool = false

    var body: some View {
        VStack(spacing: 20) {
            Text("iOS Dictionary Lookup")
                .font(.largeTitle)
                .fontWeight(.bold)

            TextField("Enter a word", text: $word)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .padding()
                .autocapitalization(.none)
                .disableAutocorrection(true)

            Button(action: {
                if UIReferenceLibraryViewController.dictionaryHasDefinition(forTerm: word) {
                    showDefinition.toggle()
                } else {
                    print("No definition found")
                }
            }) {
                Text("Look Up")
                    .frame(maxWidth: .infinity)
                    .padding()
                    .background(Color.blue)
                    .foregroundColor(.white)
                    .cornerRadius(10)
            }
            .disabled(word.isEmpty)
            .padding()

            Spacer()
        }
        .padding()
        .sheet(isPresented: $showDefinition) {
            DictionaryViewControllerWrapper(word: word)
        }
    }
}

struct DictionaryViewControllerWrapper: UIViewControllerRepresentable {
    let word: String

    func makeUIViewController(context: Context) -> UIReferenceLibraryViewController {
        UIReferenceLibraryViewController(term: word)
    }

    func updateUIViewController(_ uiViewController: UIReferenceLibraryViewController, context: Context) {}
}

struct DictionaryView_Previews: PreviewProvider {
    static var previews: some View {
        DictionaryView()
    }
}
